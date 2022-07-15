import React from "react";
import { Logo } from "./common";

import './common.css';
import './Footer.css';

function Footer(): JSX.Element {
  return (<>
    <div className="footer">
      <div className="footer-inner">
        <Logo size="120px" fontSize="70px"/>
        <span className="v-div" />
        <div>
        <div className="oocw-button"> button1</div>
        <div className="oocw-button"> button2</div>
        <div className="oocw-button"> button3</div>
        <div className="oocw-button"> button4</div>

        </div>
        <span className="v-div" />
        <div>info</div>
        <span className="v-div" />
        <div>links</div>
      </div>
    </div>
  </>);
}

export default Footer;