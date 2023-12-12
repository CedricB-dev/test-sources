import React, {useState} from 'react';
import './NavMenu.css';
import {Link} from "react-router-dom";
import {Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink} from "reactstrap";
import {useAuth} from "react-oidc-context";

export const NavMenu: React.FC = () => {
    const [collapsed, setCollapsed] = useState<boolean>(true);
    const auth = useAuth();

    return (
        <header>
            <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
                <Container>
                    <NavbarBrand tag={Link} to="/">Identity react app</NavbarBrand>
                    <NavbarToggler onClick={() => setCollapsed(p => !p)} className="mr-2"/>
                    <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!collapsed} navbar>
                        <ul className="navbar-nav flex-grow">
                            <NavItem>
                                <NavLink tag={Link} className="text-dark" to="/">Home</NavLink>
                            </NavItem>
                            <NavItem>
                                <NavLink tag={Link} className="text-dark" to="/counter">Counter</NavLink>
                            </NavItem>
                            {(auth.isAuthenticated)
                                ? <>
                                    <NavItem>
                                        <NavLink tag={Link} className="text-dark" to="/fetch-data">Fetch data</NavLink>
                                    </NavItem>
                                    <NavItem>
                                        <NavLink tag={Link} onClick={() => auth.signoutRedirect()}>Log out</NavLink>
                                    </NavItem>
                                </>
                                : <>
                                    <NavItem>
                                        <NavLink tag={Link} onClick={() => auth.signinRedirect()}>Log in</NavLink>
                                    </NavItem>
                                </>
                            }
                        </ul>
                    </Collapse>
                </Container>
            </Navbar>
        </header>
    );
};