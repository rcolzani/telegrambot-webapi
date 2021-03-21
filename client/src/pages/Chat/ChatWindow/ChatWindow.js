import React from 'react';

import Message from './Message';

const ChatWindow = (props) => {
    const chat = props.chat
        .map(m => <Message
            key={Date.now() * Math.random()}
            user={m.user}
            message={m.message}
            dataHora={m.dataHora} />);
    return (
        <div>
            {chat.length <= 0 ? <span>Aguardando mensagens...</span> : chat}
        </div>
    )
};

export default ChatWindow;