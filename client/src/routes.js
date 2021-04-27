import React from 'react';
import { BrowserRouter, Switch, Route } from 'react-router-dom';

import Chat from './pages/Chat';
import Actions from './pages/Actions';

export default function Routes() {
    return (
        <BrowserRouter>
            <Switch>
                <Route path="/" exact component={Chat} />
                <Route path="/actions" component={Actions} />
            </Switch>
        </BrowserRouter>
    );
}
