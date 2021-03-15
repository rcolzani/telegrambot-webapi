import React from 'react';
// import { Bar } from 'react-chartjs-2';
import { Container, Title } from './styles';


// const data = {
//     labels: ['1', '2', '3', '4', '5', '6'],
//     datasets: [
//         {
//             label: '# of Votes',
//             data: [12, 19, 3, 5, 2, 3],
//             fill: false,
//             color: '#fff',
//             backgroundColor: 'rgb(255, 99, 132)',
//             borderColor: 'rgba(255, 99, 132, 0.2)',
//         },
//     ],
// }

const Dashboard = () => {
    return (
        <Container>
            <Title>Dashboard</Title>
            {/* <Bar
                data={data}
                width={'100%'}
                height={'100%'}
                options={{ maintainAspectRatio: false }}
            /> */}
        </Container>
    );
};

export default Dashboard;
