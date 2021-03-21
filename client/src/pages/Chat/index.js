import React, { useState, useEffect, useRef } from 'react';
import { HubConnectionBuilder, HttpTransportType } from '@microsoft/signalr';

import ChatWindow from './ChatWindow/ChatWindow';
import { Title, Container, ServerStatus, Messages } from './styles';

const Chat = () => {
    const [chat, setChat] = useState([]);
    const [serverStatus, setServerStatus] = useState("");
    const latestChat = useRef(null);

    latestChat.current = chat;

    useEffect(() => {
        const connection = new HubConnectionBuilder()
            .withUrl(`${process.env.REACT_APP_SERVER_URL}/hubs/chat`)
            .withAutomaticReconnect()
            .build();

        //funcionando no heroku
        // const connection = new HubConnectionBuilder()
        //     .withUrl(`${process.env.REACT_APP_SERVER_URL}/hubs/chat`, {
        //         skipNegotiation: true,
        //         transport: HttpTransportType.WebSockets
        //     })
        //     .withAutomaticReconnect()
        //     .build();

        setServerStatus("Iniciando conexão");
        connection.start()
            .then(result => {
                setServerStatus("Conectado com sucesso")
                connection.on('ReceiveMessage', message => {
                    let updatedChat = [...latestChat.current];
                    console.log(latestChat.current.length);
                    if (latestChat.current.length >= 10) {
                        updatedChat = updatedChat.slice(1);
                    }
                    updatedChat.push(message);
                    updatedChat.sort((a, b) => b.dataHora - a.dataHora);
                    console.log(message);
                    setChat(updatedChat);
                });
            })
            .catch(e => {
                setServerStatus("Conexão falhou com o servidor")
                console.log('Connection failed: ', e)
            });
    }, []);

    return (
        <Container>
            <ServerStatus><span>Status: </span> {serverStatus}</ServerStatus>
            <Messages>
                <Title>Mensagens enviadas e recebidas</Title>
                <ChatWindow chat={chat} />
            </Messages>
        </Container>
    );
};

export default Chat;
