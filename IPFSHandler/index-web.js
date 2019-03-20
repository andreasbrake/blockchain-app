const express = require('express');

const app = express();
const $APP_PORT = process.env.PORT || 5109;

app.get('/ipfs/*', function (request, response) {
  let hash = request.params.fileId;
  ipfs.dag.get(hash, (err, obj) => {
    if(err)
    {
      return response.json(err)  
    }
    
    let resObj = obj.value
    for(let i=1; i < request.params.length; i++)
    {
      resObj = resObj[request.params[i]]
    }
    return response.json(resObj)
  })
});

app.listen($APP_PORT);
console.log("server started on port " + $APP_PORT);
