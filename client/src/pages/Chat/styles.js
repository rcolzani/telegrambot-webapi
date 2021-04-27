import styled from 'styled-components';


export const Container = styled.div`
    display: flex;
    flex-direction: column;
    align-items: center;

`
export const ServerStatus = styled.span`
    align-self: flex-start;
`
export const ServerActions = styled.span`
    align-self: flex-end;
    padding-right: 15px;
    a { color: #fefefe;}
`

export const Messages = styled.div`
padding: 0 10px;

 small{
        margin-top: 50px;
        align-self: flex-end;
    }
`

export const Title = styled.h1`
    //margin-top: 50px;
`