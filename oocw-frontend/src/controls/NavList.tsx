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
  selected?: string[]
}

class NavList extends React.Component<NavListInfo> {


  state = {
    selected: this.props.selected || []
  }

  renderSub(node: NavNode, depth: number = 0, selected: boolean = false): JSX.Element {
    const {t} = this.props;
    const children: JSX.Element[] = [];
    node.children.forEach(x => children.push(
      this.renderSub(
        x, 
        depth + 1, 
        x.key === this.state.selected[depth]
        )
      ));
    const href = `api/list?key=${node.key}&action=${node.action}`;
    // TODO assign proper address
    return <>
      <li className="list-item">
        <details className={children.length === 0 ? "unmarked" : ""} open={selected}>
          <summary>
          <a href={href}>{t(node.key)}</a>
          </summary>
          <ul className="list-root">
            {children}
          </ul>
        </details>
        
      </li>
    </>
  }

  render() {
    const rChs: JSX.Element[] = [];
    this.props.root.children.forEach(x => rChs.push(this.renderSub(x, 1, x.key === this.state.selected[0])));
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