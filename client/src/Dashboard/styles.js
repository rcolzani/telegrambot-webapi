import styled from 'styled-components';
import { shade } from 'polished';


export const Container = styled.div`
  width: 50%;
  display: flex;
  flex-direction: column;
  align-items: center;

  background-color: #06031f;
background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 1600 900'%3E%3Cpolygon fill='%235757ff' points='957 450 539 900 1396 900'/%3E%3Cpolygon fill='%23243665' points='957 450 872.9 900 1396 900'/%3E%3Cpolygon fill='%23764bef' points='-60 900 398 662 816 900'/%3E%3Cpolygon fill='%23303672' points='337 900 398 662 816 900'/%3E%3Cpolygon fill='%238a3ede' points='1203 546 1552 900 876 900'/%3E%3Cpolygon fill='%2342337d' points='1203 546 1552 900 1162 900'/%3E%3Cpolygon fill='%23992fcd' points='641 695 886 900 367 900'/%3E%3Cpolygon fill='%23582d85' points='587 900 641 695 886 900'/%3E%3Cpolygon fill='%23a31ebb' points='1710 900 1401 632 1096 900'/%3E%3Cpolygon fill='%236f2288' points='1710 900 1401 632 1365 900'/%3E%3Cpolygon fill='%23aa00aa' points='1210 900 971 687 725 900'/%3E%3Cpolygon fill='%23880088' points='943 900 1210 900 971 687'/%3E%3C/svg%3E");
background-attachment: fixed;
background-size: cover;
`;

export const Title = styled.h1`
    color: #fff;
`
export const Content = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  place-content: center;

  width: 100%;
  max-width: 700px;

  form {
    margin: 80px 0;
    width: 340px;
    text-align: center;

    h1 {
      margin-bottom: 24px;
    }

    a {
      color: #f4ede8;
      display: block;
      margin-top: 24px;
      text-decoration: none;
      transition: color 0.2s;

      &:hover {
        color: ${shade(0.2, '#f4ede8')};
      }
    }
  }

  > a {
    color: #ff9000;
    margin-top: 24px;
    text-decoration: none;
    transition: color 0.2s;

    display: flex;
    align-items: center;

    &:hover {
      color: ${shade(0.2, '#ff9000')};
    }

    svg {
      margin-right: 16px;
    }
  }
`;

