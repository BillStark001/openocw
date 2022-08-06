import { Request, Response } from "express";
import express from 'express';

// local
import config from './config';


// init
const app: express.Express = express();
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

app.listen(config.port, () => {
  console.log(`Start on port ${config.port}.`);
});

