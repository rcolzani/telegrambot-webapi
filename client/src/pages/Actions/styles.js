import styled from 'styled-components';


export const Container = styled.div`
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    
    a{ 
        margin: 10px;
        color: #eee; 
        align-self: flex-start;
        text-decoration: none;
    }
`
export const Title = styled.span`
margin: 10px 0;
font-size: 25px;
padding-top: 50px;
`

export const PasswordInputGroup = styled.div`
    border-radius: 5px;
    background: #fff;
    padding: 10px;
    color: #000;
`
export const PasswordInput = styled.input`
    border: none;
    padding-left: 20px;
`

export const ChangeServerStatusButton = styled.button`
    margin-top: 10px;
    padding: 10px;
    border-radius: 5px;
  
    width: 100px;
    border: solid 2px;
    border-color:  ${props => (props.CurrentStatus ? '#ff704d' : '#80ff80')};
    background: #131219;
    color: #eee; 
    transition: background-color 0.3s;

    &:hover{
    background-color:${props => (props.CurrentStatus ? "#ff704d" : "#80ff80")};
    color: #131219;
}
`