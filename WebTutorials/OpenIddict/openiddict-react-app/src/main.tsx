import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App.tsx';
import 'bootstrap/dist/css/bootstrap.min.css';
import './index.css';
import {AuthProvider} from "react-oidc-context";
import {UserManagerSettings} from "oidc-client-ts";
import {BrowserRouter} from "react-router-dom";

const oidcConfig : UserManagerSettings = {
    authority: 'https://localhost:7279',
    client_id: 'react-app',
    client_secret: 'react-app-secret',
    redirect_uri: 'http://localhost:5173/signin-oidc',
    response_type: 'code',
    scope: 'openid profile roles api.read offline_access',
    post_logout_redirect_uri: 'http://localhost:5173/signout-oidc',
    loadUserInfo : false,
    automaticSilentRenew: true,
};

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
      <BrowserRouter>
          <AuthProvider {...oidcConfig}>
              <App />
          </AuthProvider>
      </BrowserRouter>
  </React.StrictMode>,
)
