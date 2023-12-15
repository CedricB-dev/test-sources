import React, {useEffect, useState} from 'react';
import {User} from "oidc-client-ts";


type forecast = {
    date: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
}


function getUser() {
    const oidcStorage = sessionStorage.getItem(`oidc.user:https://localhost:5110:react-app`)
    if (!oidcStorage) {
        return null;
    }

    return User.fromStorageString(oidcStorage);
}

export const FetchData: React.FC = () => {
    const [forecasts, setForecasts] = useState<forecast[]>([]);
    const [loading, setLoading] = useState<boolean>(true);

    useEffect(() => {
        const fetchData = async () => {
            const user = getUser();
            const token = user?.access_token;
            const response = await fetch('https://localhost:7201/weatherforecast', {
                headers: {
                    Authorization: `Bearer ${token}`,
                }
            });
            const data = await response.json();
            setForecasts(data);
            setLoading(false);
        }
        fetchData();
    }, [])

    if (loading) return <p><em>Loading...</em></p>

    return (
        <table className='table table-striped' aria-labelledby="tabelLabel">
            <thead>
            <tr>
                <th>Date</th>
                <th>Temp. (C)</th>
                <th>Temp. (F)</th>
                <th>Summary</th>
            </tr>
            </thead>
            <tbody>
            {forecasts.map(forecast =>
                <tr key={forecast.date}>
                    <td>{forecast.date}</td>
                    <td>{forecast.temperatureC}</td>
                    <td>{forecast.temperatureF}</td>
                    <td>{forecast.summary}</td>
                </tr>
            )}
            </tbody>
        </table>
    );
};