// TODO allow file access

const config = () => {
  return {
    port: 3000
  };
};

const c = config();

Object.freeze(c);

export default c;