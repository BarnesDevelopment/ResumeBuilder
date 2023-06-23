require("dotenv").config();
const express = require('express');

const app = express();

// Middleware
app.use(express.json());

// Redirect requests to endpoint starting with /api/ex to exampleController.js
app.use("/api/dbex", require("./routes/exampleRoutes"))
app.use("/api/courses",require("./routes/exampleCoursesRoutes"))

// Gobal Error Handler
app.use((err, req, res, next) => {
  console.log(err.stack);
  console.log(err.name);
  console.log(err.code);

  res.status(500).json({
    message: "Something went realy wrong",
  });
});


// Listen on pc port
const port = process.env.PORT || 3000
app.listen(port, () => console.log(`Listening on PORT ${port}`));