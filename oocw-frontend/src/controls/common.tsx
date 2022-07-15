import React from "react";

import './common.css';
import svgLogo from '../assets/svg/logo.svg';

function Logo(props:
  {
    size?: number | string,
    fontSize?: number | string, 
    vertical?: boolean
  }): JSX.Element {
  return (<div style={{
    display: "flex",
    flexDirection: props.vertical ? "column" : "row",
    color: "#FFFFFF",
    fontSize: props.fontSize || props.size || "32px",
    justifyContent: "center",
    alignItems: "center",
    fontWeight: 300,
  }}>
    <img src={svgLogo} alt="" height={props.size || "32px"} style={{ filter: "invert(100%)" }}></img>
    <span style={{ paddingLeft: "5px" }}>OpenOCW</span>
  </div>)
}

export {
  Logo,
}