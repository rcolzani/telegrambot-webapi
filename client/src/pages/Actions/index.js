import React, { useState, useEffect } from 'react';
import api from '../../services/api';
import { toast } from 'react-toastify';
import { Container, Title, PasswordInputGroup, PasswordInput, ChangeServerStatusButton } from './styles';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faUnlockAlt } from '@fortawesome/free-solid-svg-icons'
import { Link } from 'react-router-dom'

const Actions = () => {
    const [serverCurrentStatus, setServerCurrentStatus] = useState(false);
    const [password, setPassword] = useState("");

    useEffect(() => {
        async function getStatus() {
            api.get().then((response) => {
                setServerCurrentStatus(response.data);
            });
        }
        getStatus();
    }, []);

    async function handleStatus() {

        if (!password) {
            toast.error("Senha não informada.");
            return;
        }

        const dataPost = {
            "Activate": !serverCurrentStatus,
            "passwd": password
        }
        try {
            const response = await api.post('', dataPost);
            setServerCurrentStatus(response.data);
            toast.success(`Server ${response.data ? "Ativado" : "Desativado"} com sucesso`);
        } catch (err) {
            toast.error(err.response.data);
        }
    }

    return (
        <Container>
            <Link to="./">Voltar</Link>
            <Title>O server está <strong>{serverCurrentStatus ? "ativado" : "desativado"}</strong> no momento </Title>
            <span>Deseja mudar o seu estado?</span>
            <PasswordInputGroup>
                <FontAwesomeIcon icon={faUnlockAlt} />
                <PasswordInput onChange={e => setPassword(e.target.value)} placeholder="Senha" type="password" />
            </PasswordInputGroup>
            <ChangeServerStatusButton CurrentStatus={serverCurrentStatus} onClick={handleStatus}>{serverCurrentStatus ? "Desativar" : "Ativar"}</ChangeServerStatusButton>
        </Container>
    );
}

export default Actions;