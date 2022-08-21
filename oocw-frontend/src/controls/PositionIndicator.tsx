import React from 'react';
import { useTranslation } from 'react-i18next';
import './PositionIndicator.css';

interface Position {
  target?: string[]
}

export function PositionIndicator(props: Position) {
  const { t, i18n } = useTranslation();
  const target = props.target || [];

  const targetRendered: JSX.Element[] = [];
  const elemPtr = <span className="pos-ptr">{">"}</span>
  for (var key of target) {
    // TODO set proper target
    let elem = <span className="pos-txt"><a href='/'>{t(key)}</a></span>
    targetRendered.push(elemPtr);
    targetRendered.push(elem);
  }

  return <>
    <div id="pos-indicator">
      <span className="pos-txt">{t('pos.current')}</span>
      <span className="pos-txt"><a href='/'>{t('pos.root')}</a></span>
      {targetRendered}
    </div>
  </>
}