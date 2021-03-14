import React, { useState, useEffect, useRef } from 'react';
import { HubConnectionBuilder, HttpTransportType } from '@microsoft/signalr';

import ChatWindow from './ChatWindow/ChatWindow';
import ChatInput from './ChatInput/ChatInput';

const Chat = () => {
    const [chat, setChat] = useState([]);
    const latestChat = useRef(null);

    latestChat.current = chat;

    useEffect(() => {
        const connection = new HubConnectionBuilder()
            .withUrl(`${process.env.REACT_APP_SERVER_URL}/hubs/chat`, {
                skipNegotiation: true,
                transport: HttpTransportType.WebSockets
            })
            .withAutomaticReconnect()
            .build();

        console.log(`Passou: ${connection.baseUrl}`);
        connection.start()
            .then(result => {
                console.log('Connected!');

                connection.on('ReceiveMessage', message => {
                    const updatedChat = [...latestChat.current];
                    updatedChat.push(message);
                    console.log(message);
                    setChat(updatedChat);
                });
            })
            .catch(e => console.log('Connection failed: ', e));
    }, []);

    return (
        <div>
            <ChatInput />
            <hr />
            <ChatWindow chat={chat} />
        </div>
    );
};

export default Chat;
