import React, { useState, useEffect, useRef } from 'react';
import { HubConnectionBuilder, HttpTransportType } from '@microsoft/signalr';

import ChatWindow from './ChatWindow/ChatWindow';
import ChatInput from './ChatInput/ChatInput';

const Chat = () => {
    const [chat, setChat] = useState([]);
    const latestChat = useRef(null);

    latestChat.current = chat;

    useEffect(() => {
        const urlServer = 'https://bottelegramrcolzani.herokuapp.com';
        //const urlServer = 'http://localhost:5000';
        const connection = new HubConnectionBuilder()
            .withUrl(`${urlServer}/hubs/chat`, {
                skipNegotiation: true,
                transport: HttpTransportType.WebSockets
            })
            .withAutomaticReconnect()
            .build();

        console.log(`Passou: ${urlServer}/hubs/chat`);
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
