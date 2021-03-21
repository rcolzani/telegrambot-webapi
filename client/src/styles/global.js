import { createGlobalStyle } from 'styled-components';

export default createGlobalStyle`
*{
  margin: 0;
  padding: 0;
  box-sizing: border-box;
  outline: 0;
}

body{
    background-color: #06031f;
background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 1600 900'%3E%3Cpolygon fill='%235757ff' points='957 450 539 900 1396 900'/%3E%3Cpolygon fill='%23243665' points='957 450 872.9 900 1396 900'/%3E%3Cpolygon fill='%23764bef' points='-60 900 398 662 816 900'/%3E%3Cpolygon fill='%23303672' points='337 900 398 662 816 900'/%3E%3Cpolygon fill='%238a3ede' points='1203 546 1552 900 876 900'/%3E%3Cpolygon fill='%2342337d' points='1203 546 1552 900 1162 900'/%3E%3Cpolygon fill='%23992fcd' points='641 695 886 900 367 900'/%3E%3Cpolygon fill='%23582d85' points='587 900 641 695 886 900'/%3E%3Cpolygon fill='%23a31ebb' points='1710 900 1401 632 1096 900'/%3E%3Cpolygon fill='%236f2288' points='1710 900 1401 632 1365 900'/%3E%3Cpolygon fill='%23aa00aa' points='1210 900 971 687 725 900'/%3E%3Cpolygon fill='%23880088' points='943 900 1210 900 971 687'/%3E%3C/svg%3E");
background-attachment: fixed;
background-size: cover;
color: #FFF;
-webkit-font-smoothing: antialiased;
}

body, input, button{
  /* font-family: 'Roboto Slab', serif; */
  font-size: 16px;
}
h1,h2,h3,h4,h5,h6, strong{
  font-weight: 500;
}

button {cursor: pointer}

`;
