import React from 'react';
import { Container } from './styles';
import getFormattedDate from '../../../../utils/getFormattedDate';

const Message = (props) => {

    return (<Container>
        <div><p>
            <strong>{getFormattedDate(props.dataHora)} {props.user}:</strong>
        </p><p>{props.message}</p></div>
        <span style={{ background: "#250" }}></span>
    </Container>
    )
};


export default Message;