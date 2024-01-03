import React from 'react';
import { NavMenu } from './NavMenu';
import {Container} from "reactstrap";

type Props = {
    children?: React.ReactNode
};

export const Layout : React.FC<Props> = ({children}) => {
    return (
        <div>
            <NavMenu />
            <Container>
                {children}
            </Container>
        </div>
    );
};