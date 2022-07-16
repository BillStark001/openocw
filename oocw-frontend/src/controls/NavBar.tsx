import React from 'react';
import svgLogo from '../assets/svg/logo.svg';
import svgLang from '../assets/svg/lang.svg';
import svgGitHub from '../assets/svg/github.svg';
import './common.css';
import './NavBar.css';
import SearchBar from './SearchBar';
import { withTranslation, WithTranslation } from 'react-i18next';

interface NavBarNotice extends WithTranslation {
  page?: number;
  children?: JSX.Element
}

class NavBar extends React.Component<NavBarNotice> {

  element: HTMLElement | null = null;

  state = {
    height: 50, 
  }

  updateDimensions = () => {
    var size = this.element?.getBoundingClientRect();
    this.setState({
      height: this.element?.offsetHeight || 50
    });
  };
  componentDidMount() {
    window.addEventListener('resize', this.updateDimensions);
    window.addEventListener('scroll', this.updateDimensions);
    this.updateDimensions();
  }
  componentWillUnmount() {
    window.removeEventListener('resize', this.updateDimensions);
    window.removeEventListener('scroll', this.updateDimensions);
  }

  render() {
    var i = (x: number) => x == this.props.page ? "item active" : "item";
    const {t} = this.props;
    return (
      <>

        <div id="navbar-occ" style={{
          width: "100%",
          height: this.state.height, 
        }}>

        </div>
        <header id="navbar" ref={(e) => { this.element = e; }}>
          <div className="noticebar">
            {this.props.children}
          </div>
          <div className="navbar-main">
            <ul className="navbar-inner">
              <li>
                <a className="item" href="/">
                  <img src={svgLogo} style={{ marginRight: '4px' }} />
                  <span className="logo-text">{t('product.name')}</span>
                </a>
              </li>

              <li className="navbar-titles">
                <a className={i(0)} href="/docs/getting-started.html">{t('navbar.homepage')}</a>
                <a className={i(1)} href="/tutorial/tutorial.html">{t('navbar.database')}</a>
                <a className={i(2)} href="/blog/">{t('navbar.discussion')}</a>
                <a className={i(3)} href="/community/support.html">{t('navbar.info')}</a>
              </li>

              <li className="align-right">
                <a className="item subtle" href="/versions">{t("product.version")}</a>
                <a className="item" href="/languages">
                  <img src={svgLang}></img>
                </a>
                <a className="item" href="https://github.com/BillStark001/openocw" target="_blank" rel="noopener">
                  <img src={svgGitHub}></img>
                </a>
              </li>

              <li className="search-bar align-right">
                <SearchBar />

              </li>


            </ul>
          </div>
        </header>
      </>
    );
  }
}


export default withTranslation()(NavBar);