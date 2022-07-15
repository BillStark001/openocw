import React from "react";
import { createRef } from "react";


import './common.css';
import './SearchBar.css';

class SearchBar extends React.Component {

    state = {
        expand: false,
        subList: [],
        input: "",
    }

    handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        this.setState({
            input: e.target.value
        });
    }

    render() {
        return (
            <form className="search-bar">
                <span className=""
                    style={{ position: 'relative', display: 'inline-block', direction: 'ltr' }}>
                    <input 
                        type="search" 
                        id="algolia-doc-search"
                        value={this.state.input}
                        onChange={this.handleInputChange}
                        placeholder="搜索" aria-label="在文档中搜索..." className="css-7dpbpx ds-input" autoComplete="off" spellCheck="false"
                        role="combobox" aria-autocomplete="list" aria-expanded="false" aria-labelledby="algolia-doc-search"
                        aria-owns="algolia-autocomplete-listbox-0" dir="auto" style={{ position: "relative", verticalAlign: "top" }} />
                    <pre aria-hidden="true"
                        style={{ position: "absolute", visibility: "hidden", whiteSpace: "pre", fontFamily: "-apple-system, BlinkMacSystemFont, &quot;Segoe UI&quot;, Roboto, Oxygen, Ubuntu, Cantarell, &quot;Fira Sans&quot;, &quot;Droid Sans&quot;, &quot;Helvetica Neue&quot;, sans-serif; font-size: 16px; font-style: normal; font-variant: normal; font-weight: 300; word-spacing: 0px; letter-spacing: normal; text-indent: 0px; text-rendering: auto; text-transform: none" }}>ddd</pre>
                    <span className="ds-dropdown-menu ds-without-1" role="listbox" id="algolia-autocomplete-listbox-0"
                        style={{ position: "absolute", top: "100%", zIndex: 100, left: "0px", right: "auto", display: "none" }}>
                        <div className="ds-dataset-1"></div>
                    </span>
                </span>
            </form>

        );
    }
}

export default SearchBar;