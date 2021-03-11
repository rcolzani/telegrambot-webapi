import React from 'react';

const Message = (props) => (
    <div style={{ background: "#eee", borderRadius: '5px', padding: '0 10px' }}>
        {props.user == localStorage.getItem('userName') ?
            <div style={{ textAlign: 'right' }}><p><strong>{props.user}</strong> says:</p><p>{props.message}</p></div> :
            <div><p><strong>{props.user}</strong> says:</p><p>{props.message}</p></div>}
        <span style={{ background: "#250" }}>{props.datahora}</span>
    </div>
);

export default Message;