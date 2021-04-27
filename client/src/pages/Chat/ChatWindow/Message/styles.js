import styled from 'styled-components';
import { keyframes } from 'styled-components'

const breatheAnimation = keyframes`
 from {
    opacity: 0;
  }
  to {
    opacity: 0.9;
  }
`
export const Container = styled.div`
 opacity: 0.9;
 margin: 15px 0;
 padding: 15px;
 max-width: 580px;
 border-radius: 5px;
 background: #ffffff10;

 animation-name: ${breatheAnimation};
 animation-duration: 1s;

 strong{
     font-size: 1.2rem;
     border-bottom: solid 1px #ffffff50;
 }
`