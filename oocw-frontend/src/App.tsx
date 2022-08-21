import React from 'react';
import logo from './logo.svg';
import './general.css';

import NavBar from './controls/NavBar';
import PageFrame from './pages/PageFrame';
import Footer from './controls/Footer';
import navList from './assets/meta/orgtree.json'

import { useTranslation, withTranslation } from 'react-i18next';
import { NavList, NavNode } from './controls/NavList';
import { PositionIndicator } from './controls/PositionIndicator';

function App() {
  const { t } = useTranslation();
  const nList = navList as NavNode;

  const curTarget = ["org.soe", "crs.u", "org.m.u.sce", "1145141919810"];

  return (
    <>
      <NavBar page={0}>
        <div>test notice</div>

      </NavBar>

      <PageFrame
        left={
          <div>
            <NavList 
            root={nList}
            selected={curTarget}
            ></NavList>
          </div>
        }
        right={<div>right</div>}
        top={
          <>
          
          <div style={{ height: "200px" }}>
            some sort of banner? 114514
          </div>
          <PositionIndicator target={curTarget}></PositionIndicator>  
          </>
        }
      >
        <div className="App">
          <header className="App-header">
            <img src={logo} className="App-logo" alt="logo" />
            <p>
              Edit <code>src/App.tsx</code> and save to reload.
            </p>
            <a
              className="App-link"
              href="https://reactjs.org"
              target="_blank"
              rel="noopener noreferrer"
            >
              Learn React
            </a>
          </header>
        </div>
        <div>test translation</div>
        <div>org.tokyotech</div>
        <div>{t('crs.u')}</div>
        <div>test</div>
        <div>test</div>
        <div>test</div>
        <div>test</div>

        <div>test</div>
        <div>test</div>
        <div>test</div>
        <div>test</div>
        <div>test</div>
        <div>test</div>
        <div>test</div>
        <div>test</div>
      </PageFrame>
      <Footer></Footer>
    </>
  );
}

export default withTranslation()(App);
