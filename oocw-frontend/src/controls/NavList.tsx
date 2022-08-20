import React from "react";
import { withTranslation, WithTranslation } from 'react-i18next';
import './common.css';
import './NavList.css';


export interface NavNode {
  key: string;
  action: string;
  children: NavNode[];
}

export interface NavListInfo extends WithTranslation {
  root: NavNode,
  children?: JSX.Element | JSX.Element[],
}

class NavList extends React.Component<NavListInfo> {


  state = {
    selected: "none"
  }

  renderSub(node: NavNode): JSX.Element {
    const {t} = this.props;
    const children: JSX.Element[] = [];
    node.children.forEach(x => children.push(this.renderSub(x)))
    const href = `api/list?key=${node.key}&action=${node.action}`;
    // TODO assign proper address
    return <>
      <li className="list-item">
        <a href={href}>{t(node.key)}</a>
        <ul className="list-root">
          {children}
        </ul>
      </li>
    </>
  }

  render() {
    const rChs: JSX.Element[] = [];
    this.props.root.children.forEach(x => rChs.push(this.renderSub(x)));
    return <>
      <ul id="navlist">
        {rChs}
      </ul>
    
    
    </>
  }
}

const _nl1 = withTranslation()(NavList);

export {
  _nl1 as NavList
};