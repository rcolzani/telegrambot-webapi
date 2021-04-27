import React, { useState, useEffect, useRef } from 'react';
import { HubConnectionBuilder, HttpTransportType } from '@microsoft/signalr';
import { Link } from 'react-router-dom';
import { toast } from 'react-toastify';
import ChatWindow from './ChatWindow';
import Actions from '../Actions'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faCogs } from '@fortawesome/free-solid-svg-icons'
import { Title, Container, ServerStatus, ServerActions, Messages } from './styles';

const Chat = () => {
    const [chat, setChat] = useState([]);
    const [serverStatus, setServerStatus] = useState("");
    const latestChat = useRef(null);

    latestChat.current = chat;

    const statusServerEnum = { "Ok": 1, "Falhou": 2, "Sucesso": 3 }

    const updateServerStatus = (description, statusServer) => {
        setServerStatus(description);
        switch (statusServer) {
            case statusServerEnum.Sucesso:
                toast.success(description);
                break;
            case statusServerEnum.Falhou:
                toast.error(description);
                break;
            default:
                toast(description);
                break;
        }
    }

    useEffect(() => {
        document.title = "TelegramBot - Mensagens";

        const urlServer = process.env.NODE_ENV === 'development' ? process.env.REACT_APP_SERVER_URL_LOCAL : process.env.REACT_APP_SERVER_URL_HEROKU;

        const connection = new HubConnectionBuilder()
            .withUrl(`${urlServer}/hubs/chat`)
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

        updateServerStatus("Iniciando conexão", statusServerEnum.Ok);
        connection.start()
            .then(result => {
                updateServerStatus("Conectado com sucesso", statusServerEnum.Sucesso)
                connection.on('ReceiveMessage', message => {
                    let updatedChat = [...latestChat.current];
                    console.log(latestChat.current.length);
                    if (latestChat.current.length >= 6) {
                        updatedChat.pop();// = updatedChat.slice(5);
                    }
                    updatedChat.unshift(message);
                    updatedChat.sort((a, b) => b.dataHora - a.dataHora);
                    console.log(message);
                    setChat(updatedChat);
                });
            })
            .catch(e => {
                updateServerStatus("Conexão falhou com o servidor", statusServerEnum.Falhou)
                console.log('Connection failed: ', e)
            });
    }, []);

    return (
        <Container>
            <ServerStatus><span>Status: </span> {serverStatus}</ServerStatus>
            <ServerActions>
                <Link to="/actions"> <FontAwesomeIcon icon={faCogs} size="2x" /></Link>
            </ServerActions>
            <Messages>
                <Title>Mensagens enviadas e recebidas</Title>
                <ChatWindow chat={chat} />
                <small>Os nomes dos usuários são limitados a 2 caracteres para privacidade</small>
            </Messages>
        </Container>
    );
};

export default Chat;
