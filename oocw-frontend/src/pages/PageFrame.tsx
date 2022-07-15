import React from "react";

import './common.css';
import './PageFrame.css';


const SIDEBAR_SIZE_L = 200;
const SIDEBAR_SIZE_R = 300
const CONTENT_MIN_SIZE = 600;

enum PanelState {
  Top = 0, Middle = 1, Bottom = 2
}

function generateFloatingStyle(state: PanelState): React.CSSProperties {
  if (state === PanelState.Top)
    return { position: "static" };
  else if (state === PanelState.Bottom)
    return { position: "absolute", bottom: "0px" };
  else
    return { position: "fixed", top: "0px" };
}

interface PageContent {
  left?: JSX.Element,
  children?: JSX.Element | JSX.Element[],
  right?: JSX.Element,
  top?: JSX.Element,
}

class PageFrame extends React.Component<PageContent> {

  element: HTMLElement | null = null;
  middle: HTMLElement | null = null;

  state = {
    leftActive: true,
    rightActive: true,
    floatState: PanelState.Top,

    width: 1280,
    height: 600,
    y: 0,
  }

  handleResize = () => {

  }

  render() {
    var sizeL = this.props.left != null ? SIDEBAR_SIZE_L : 0;
    var sizeR = this.props.right != null ? SIDEBAR_SIZE_R : 0;
    var sizeLR = Math.max(sizeL, sizeR);
    var row = 1;
    if (this.state.width > CONTENT_MIN_SIZE + sizeL + sizeR)
      row = 3;
    else if (this.state.width > CONTENT_MIN_SIZE + sizeLR)
      row = 2;
    return (
      <>
        <div className="page-frame">
          <div id="pf-top">
            {this.props.top}
          </div>
          <div ref={(e) => { this.element = e; }} id="pf-inner">

            {row > 1 ?
              <div id="pf-left" style={Object.assign(generateFloatingStyle(this.state.floatState), { width: row === 2 ? sizeLR : sizeL })}>

                {this.state.width}<br />
                {this.state.height}<br />
                {this.state.y}<br />
                {this.props.left}
                {row === 2 ? <><br />{this.props.right}</> : null}
              </div>
              : null
            }
            <div id="pf-middle" ref={(e) => { this.middle = e; }}
              style={{
                width: this.state.width - [0, 0, sizeLR, sizeL + sizeR][row],
                // left: [0, 0, sizeLR, sizeL][row]
              }}>
              {this.props.children}
            </div>
            {
              row === 3 ?
                <div id="pf-right" style={generateFloatingStyle(this.state.floatState)}>
                  {this.props.right}
                </div>
                : null
            }
          </div>
        </div>
      </>
    );
  }

  updateDimensions = () => {
    var size = this.element?.getBoundingClientRect();
    this.setState({
      height: Math.max(this.middle?.offsetHeight || 0, size?.height || 0),
      width: size?.width,
      y: size?.top,
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

}

export default PageFrame;