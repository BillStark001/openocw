import React from "react";

import './common.css';
import svgLogo from '../assets/svg/logo.svg';
import { withTranslation, WithTranslation } from 'react-i18next';

interface LogoPropsScheme extends WithTranslation {
    size?: number | string,
    fontSize?: number | string, 
    vertical?: boolean
  }

function Logo_(props: LogoPropsScheme
  ): JSX.Element {
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
    <span style={{ paddingLeft: "5px" }}>{props.t('product.name')}</span>
  </div>)
}
export const Logo = withTranslation()(Logo_);
