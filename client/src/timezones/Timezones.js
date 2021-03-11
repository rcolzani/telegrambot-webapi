import React, { useState, useEffect } from 'react';
import { HubConnectionBuilder } from '@microsoft/signalr';

const Timezones = () => {
    const [timezones, setTimezones] = useState({});
    const urlServer = "https://chattyrcolzani.herokuapp.com";
    // const urlServer = "http://localhost:5000";
    useEffect(() => {
        const connection = new HubConnectionBuilder()
            .withUrl(`${urlServer}/hubs/chat`)
            .withAutomaticReconnect()
            .build();

        console.log(`Passou: ${urlServer}/hubs/chat`);
        connection.start()
            .then(result => {
                console.log('Connected!');

                connection.on('TimezoneChange', tzs => {
                    console.log(tzs);
                    setTimezones(tzs);
                });
            })
            .catch(e => console.log('Connection failed: ', e));
    }, []);

    return (
        <div>
            <span>{timezones.brazil}</span>
            <span>{timezones.usa}</span>

        </div>
    );
};

export default Timezones;
