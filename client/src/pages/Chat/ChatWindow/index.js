import React from 'react';

import Message from './Message';
import { Container } from './styles'

const ChatWindow = (props) => {
    const chat = props.chat
        .map(m => <Message
            key={Date.now() * Math.random()}
            user={m.user}
            message={m.message}
            dataHora={m.dataHora} />);
    return (
        <Container>
            {chat.length <= 0 ? <span>Aguardando mensagens...</span> : chat}
        </Container>
    )
};

export default ChatWindow;