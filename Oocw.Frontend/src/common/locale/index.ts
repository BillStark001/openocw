import { CompiledLanguageResource, compileLanguageResource } from "./model";
import * as defs from './defs';
import { deepAssign } from "@/utils/object";

const fullLanguageFile: {
  [lang: string]: CompiledLanguageResource;
} = {};

for (const lang of [
  defs.frontend,
  defs.login,
  defs.meta,
  defs.tit,
]) {
  const compiled = compileLanguageResource(lang, defs.langOrder);
  deepAssign(fullLanguageFile, compiled);
};

Object.freeze(fullLanguageFile);

export default fullLanguageFile;
