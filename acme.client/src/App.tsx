import { useEffect, useState } from 'react';
import { DataGrid, GridToolbarContainer, GridToolbarFilterButton } from '@mui/x-data-grid';
import { Button, Grid, Typography, Box, Alert, TextField } from '@mui/material';
import { styled } from '@mui/system';
import './App.css';

const Input = styled('input')({
    display: 'none',
});

function App() {
    const [subscribers, setSubscribers] = useState<any>([]);
    const [selectedFiles, setSelectedFiles] = useState([]);
    const [uploadResponse, setUploadResponse] = useState<any>();
    const [filterName, setFilterName] = useState('');
    const [filterExpirationDate, setFilterExpirationDate] = useState('');

    const fetchData = async () => {
        const response = await fetch('subscription');
        const data = await response.json();
        setSubscribers(data);
    };

    useEffect(() => {
        fetchData();
    }, []);

    const handleFileChange = (event: any) => {
        setSelectedFiles(event.target.files);
    };

    const handleSubmit = async () => {
        const formData = new FormData();
        for (let i = 0; i < selectedFiles.length; i++) {
            formData.append('files', selectedFiles[i]);
        }

        const response = await fetch('subscription', {
            method: 'POST',
            body: formData,
        });
        const result = await response.json();

        setUploadResponse(result);
        fetchData();
    };

    const columns = [
        { field: 'email', headerName: 'Email', width: 300 },
        { field: 'expirationDate', headerName: 'Expiration Date', width: 300 },
    ];

    const filteredRows = subscribers.filter((subscriber: any) => {
        return (
            subscriber.email.toLowerCase().includes(filterName.toLowerCase()) &&
            subscriber.expirationDate.includes(filterExpirationDate)
        );
    });

    return (
        <Box sx={{ p: 2 }}>
            <Typography variant="h4" gutterBottom>
                Subscribers List
            </Typography>
            <Box sx={{ mb: 2 }}>
                <TextField
                    label="Filter by Name"
                    variant="outlined"
                    size="small"
                    value={filterName}
                    onChange={(e) => setFilterName(e.target.value)}
                    sx={{ mr: 2 }}
                />
                <TextField
                    label="Filter by Expiration Date"
                    variant="outlined"
                    size="small"
                    value={filterExpirationDate}
                    onChange={(e) => setFilterExpirationDate(e.target.value)}
                />
            </Box>
            <DataGrid
                rows={filteredRows}
                columns={columns}
                autoHeight
                components={{
                    Toolbar: () => (
                        <GridToolbarContainer>
                            <GridToolbarFilterButton />
                        </GridToolbarContainer>
                    ),
                }}
            />
            <Box sx={{ mt: 4 }}>
                <Typography variant="h6" gutterBottom>
                    Import Subscribers Data
                </Typography>
                <Grid container spacing={2}>
                    <Grid item xs={12}>
                        <label htmlFor="file-input">
                            <Input
                                id="file-input"
                                type="file"
                                multiple
                                onChange={handleFileChange}
                            />
                            <Button variant="contained" component="span">
                                Select Files
                            </Button>
                        </label>
                        {selectedFiles.length > 0 && (
                            <Box sx={{ mt: 2 }}>
                                <Typography variant="body1">Selected Files:</Typography>
                                <ul>
                                    {Array.from(selectedFiles).map((file: any, index) => (
                                        <li key={index}>{file.name}</li>
                                    ))}
                                </ul>
                            </Box>
                        )}
                    </Grid>
                    <Grid item xs={12}>
                        <Button
                            variant="contained"
                            color="primary"
                            onClick={handleSubmit}
                            disabled={selectedFiles.length === 0}
                        >Submit</Button>
                    </Grid>
                    {uploadResponse && (
                        <Grid item xs={12}>
                            <Box>
                                <Alert severity="error">
                                    <Typography>Validation Errors:</Typography>
                                    <ul>
                                        {uploadResponse.errors.map((error: any, index: number) => (
                                            <li key={index}>{error.message}</li>
                                        ))}
                                    </ul>
                                </Alert>
                            </Box>
                        </Grid>
                    )}
                </Grid>
            </Box>
        </Box>
    );
}

export default App;
