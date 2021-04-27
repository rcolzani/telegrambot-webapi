import React from 'react';
import { Container } from './styles';
import getFormattedDate from '../../../../utils/getFormattedDate';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faCogs } from '@fortawesome/free-solid-svg-icons'

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