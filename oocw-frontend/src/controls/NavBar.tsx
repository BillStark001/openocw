import React from 'react';
import logo from './logo.svg';
import './NavBar.css';

function NavBar() {
    return (
        <header className="navbar">
            <div className="import-info">
                test
            </div>
            <div className="navbar-main">
                <ul className="navbar-inner">
                    <li>
                    <a className="item logo noreturn" href="/">
                        <img src="null" alt="" height="20"/>
                            <span className="logo-text">OpenOCW</span>
                    </a>
                    </li>
                    
                    <li className="titles noreturn">
                        <a className="item" href="/docs/getting-started.html">Home</a>
                        <a aria-current="page" className="item navbar-title" href="/tutorial/tutorial.html">
                            Database
                            <span className="bottom-highlight"></span>
                        </a>
                        <a className="item" href="/blog/">Discussion</a>
                        <a className="item" href="/community/support.html">Info</a>
                    </li>
                    
                    <li className="align-right noreturn">
                        <a className="item subtle" href="/versions">v0.1.0</a>
                        <a className="item" href="/languages">
                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24">
                                <path d="M0 0h24v24H0z" fill="none"></path>
                                <path
                                    d=" M12.87 15.07l-2.54-2.51.03-.03c1.74-1.94 2.98-4.17 3.71-6.53H17V4h-7V2H8v2H1v1.99h11.17C11.5 7.92 10.44 9.75 9 11.35 8.07 10.32 7.3 9.19 6.69 8h-2c.73 1.63 1.73 3.17 2.98 4.56l-5.09 5.02L4 19l5-5 3.11 3.11.76-2.04zM18.5 10h-2L12 22h2l1.12-3h4.75L21 22h2l-4.5-12zm-2.62 7l1.62-4.33L19.12 17h-3.24z "
                                    className="css-c4d79v"></path>
                            </svg> 
                            <span className="">Languages</span>
                        </a>
                        <a href="https://github.com/facebook/react/" target="_blank" rel="noopener" className="item">
                                GitHub
                                <svg x="0px" y="0px" viewBox="0 0 100 100" width="15" height="15" className="css-19vhmgv">
                                    <path fill="currentColor" d="
        M18.8,85.1h56l0,0c2.2,0,4-1.8,4-4v-32h-8v28h-48v-48h28v-8h-32l0,
        0c-2.2,0-4,1.8-4,4v56C14.8,83.3,16.6,85.1,18.8,85.1z
        "></path>
                                    <polygon fill="currentColor" points="
        45.7,48.7 51.3,54.3 77.2,28.5 77.2,37.2 85.2,37.2 85.2,14.9 62.8,
        14.9 62.8,22.9 71.5,22.9
        "></polygon>
                                </svg>
                        </a>
                    </li>
                </ul>
            </div>

        </header>
    );
}

export default NavBar;