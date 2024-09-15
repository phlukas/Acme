using Acme.Server.Controllers.Dtos.Subscription;
using Acme.Server.Data;
using Acme.Server.Data.Records;
using CsvHelper;
using CsvHelper.Configuration;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Acme.Server.Services;

public class SubscriptionService
{
    private readonly ApplicationDbContext _dbContext;

    public SubscriptionService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SubscriptionDto[]> GetAllAsync()
    {
        return await _dbContext.Subscriptions.Select(x => new SubscriptionDto() { Id = x.Id, Email = x.Email, ExpirationDate = x.ExpirationDate }).ToArrayAsync();
    }

    public async Task<Result> UploadSubscriptions(List<IFormFile> files)
    {
        var errors = new List<Error>();
        var now = DateTime.UtcNow;
        var subscriptionsToAdd = new List<SubscriptionSetDto>();
        var expiredSubscriptions = new List<SubscriptionSetDto>();
        var config = new CsvConfiguration(CultureInfo.InvariantCulture);

        if (!IsFilesValid(files, errors))
        {
            return new Result().WithErrors(errors);
        }

        foreach (var file in files)
        {
            try
            {
                using (var stream = file.OpenReadStream())
                using (var reader = new StreamReader(stream))
                using (var csv = new CsvReader(reader, config))
                {
                    var subscriptions = csv.GetRecords<SubscriptionSetDto>().ToList();
                    subscriptionsToAdd.AddRange(subscriptions.Where(x => now <= x.ExpirationDate));
                    expiredSubscriptions.AddRange(subscriptions.Where(x => now > x.ExpirationDate));
                }
            }
            catch (Exception e)
            {
                errors.Add(new Error($"Reading file '{file.FileName}' following error occurred: {e.Message}"));
            }
        }

        var validSubscriptionsToAdd = await ValidateSubscriptions(errors, subscriptionsToAdd, expiredSubscriptions);

        _dbContext.Subscriptions.AddRange(validSubscriptionsToAdd.Select(x => new SubscriptionRecord() { Email = x.Email, ExpirationDate = x.ExpirationDate }));
        await _dbContext.SaveChangesAsync();

        return errors.Count != 0 ? new Result().WithErrors(errors) : Result.Ok();
    }

    private static bool IsFilesValid(List<IFormFile> files, List<Error> errors)
    {
        if (files is null || files.Count == 0)
        {
            errors.Add(new Error("At least one file must be provided."));
            return false;
        }

        if (files.Any(file => !(Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))))
        {
            errors.Add(new Error("All files extensions must be csv."));
            return false;
        }

        return true;
    }

    private async Task<List<SubscriptionSetDto>> ValidateSubscriptions(List<Error> errors, List<SubscriptionSetDto> subscriptionsToAdd, List<SubscriptionSetDto> expiredSubscriptions)
    {
        var emailValidator = new EmailAddressAttribute();
        var validSubscriptionsToAdd = new List<SubscriptionSetDto>();

        foreach (var subscription in subscriptionsToAdd)
        {
            var errorsToAdd = new List<Error>();
            var subscriptionAlreadyExists = await _dbContext.Subscriptions.AnyAsync(x => x.Email == subscription.Email);

            if (subscriptionAlreadyExists)
            {
                errorsToAdd.Add(new Error($"Subscription with email '{subscription.Email}' already exists in the database."));
            }

            if (subscriptionsToAdd.Where(x => x.Email == subscription.Email).Count() > 1)
            {
                errorsToAdd.Add(new Error($"Subscription with email '{subscription.Email}' is provided more than once."));
            }

            if (subscription.Email.Length > 320)
            {
                errorsToAdd.Add(new Error($"Subscription email '{subscription.Email}' is longer than 320 symbols."));
            }

            if (subscription.Email.Length == 0)
            {
                errorsToAdd.Add(new Error("Subscription email cannot be empty."));
            }

            if (!emailValidator.IsValid(subscription.Email))
            {
                errorsToAdd.Add(new Error($"Subscription email '{subscription.Email}' is invalid."));
            }

            if (subscription.ExpirationDate == DateTime.MinValue)
            {
                errorsToAdd.Add(new Error($"Subscription with email '{subscription.Email}' has no expiration date."));
            }

            // Do not add subscription if there are any errors in the subscription
            if (errorsToAdd.Count == 0)
            {
                validSubscriptionsToAdd.Add(subscription);
            }

            errors.AddRange(errorsToAdd);
        }

        if (expiredSubscriptions.Count != 0)
        {
            errors.Add(new Error($"Provided file(s) contains these expired subscriptions: {string.Join(';', expiredSubscriptions.Select(x => x.Email))}."));
        }

        return validSubscriptionsToAdd;
    }
}
