import React from 'react';
import svgLogo from '../assets/svg/logo.svg';
import svgLang from '../assets/svg/lang.svg';
import svgGitHub from '../assets/svg/github.svg';
import './common.css';
import './NavBar.css';
import SearchBar from './SearchBar';

class NavBar extends React.Component {

    render() {
        return (
            <>
            
            <div id="navbar-occ">

            </div>
                <header id="navbar">
                    <div className="noticebar">
                        test
                    </div>
                    <div className="navbar-main">
                        <ul className="navbar-inner">
                            <li>
                                <a className="item" href="/">
                                    <img src={svgLogo} style={{ marginRight: '4px' }} />
                                    <span className="logo-text">OpenOCW</span>
                                </a>
                            </li>

                            <li className="titles">
                                <a className="item active" href="/docs/getting-started.html">Home</a>
                                <a className="item" href="/tutorial/tutorial.html"> Database </a>
                                <a className="item" href="/blog/">Discussion</a>
                                <a className="item" href="/community/support.html">Info</a>
                            </li>

                            <li className="align-right">
                                <a className="item subtle" href="/versions">v0.1.0</a>
                                <a className="item" href="/languages">
                                    <img src={svgLang}></img>
                                </a>
                                <a className="item" href="https://github.com/BillStark001/openocw" target="_blank" rel="noopener">
                                    <img src={svgGitHub}></img>
                                </a>
                            </li>
                            
                            <li className="search-bar align-right">
                            <SearchBar/>
                                
                            </li>

                            
                        </ul>
                    </div>
                </header>
            </>
        );
    }
}


export default NavBar;