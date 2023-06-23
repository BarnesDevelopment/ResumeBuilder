const Joi = require('joi');

const courses = [
  { id: 1, name: "course1" },
  { id: 2, name: "course2" },
  { id: 3, name: "course3" },
  { id: 4, name: "course4" },
];

function validateCourse (course) {
const courseSchema = Joi.object({
  name: Joi.string().min(3).required()
});
return courseSchema.validate(course);
}

exports.getAllCourses = async (req, res, next) => {
  res.send(courses);
}

exports.getCourseByID = async (req, res, next) => {
  const course = courses.find(c => c.id === parseInt(req.params.id));
  if (!course) return res.status(404).send('The course with the given ID was not found.');
  res.send(course);
}

exports.getCoursesByDate = async (req, res, next) => {
  res.json({"year":req.params.year,"month":req.params.month,"sortBy":req.query.sortBy});
}

exports.addNewCourse = async (req, res, next) => {
  const { error } = validateCourse(req.body);
  if (error) return res.status(400).send(result.error.details[0].message);

  const course = {
    id: courses.length + 1,
    name: req.body.name
  };
  courses.push(course);
  res.send(course);
}

exports.modifyExistingCourse = async (req, res, next) => {
  const course = courses.find(c => c.id === parseInt(req.params.id));
  if (!course) return res.status(404).send('The course with the given ID was not found.');

  const { error } = validateCourse(req.body);
  if (error) return res.status(400).send(result.error.details[0].message);

  course.name = req.body.name;
  res.send(course);
}

exports.removeCourse = async (req, res, next) => {
  const course = courses.find(c => c.id === parseInt(req.params.id));
  if (!course) return res.status(404).send('The course with the given ID was not found.');

  const index = courses.indexOf(course);
  courses.splice(index, 1);

  res.send(course);
}

/*const courses = [
    { id: 1, name: "course1" },
    { id: 2, name: "course2" },
    { id: 3, name: "course3" },
    { id: 4, name: "course4" },
  ];
  
  function validateCourse (course) {
    const courseSchema = Joi.object({
      name: Joi.string().min(3).required()
    });
    return courseSchema.validate(course);
  }
  
  app.get('/', (req, res) => {
    res.send('Hello World');
  });
  
  app.get('/api/courses', (req, res) => {
    res.send(courses);
  });
  
  app.get('/api/courses/:id', (req, res) => {
    const course = courses.find(c => c.id === parseInt(req.params.id));
    if (!course) return res.status(404).send('The course with the given ID was not found.');
    res.send(course);
  });
  
  app.get('/api/posts/:year/:month', (req, res) => {
    res.json({"year":req.params.year,"month":req.params.month,"sortBy":req.query.sortBy});
  });
  
  app.post('/api/courses', (req, res) => {
    const { error } = validateCourse(req.body);
    if (error) return res.status(400).send(result.error.details[0].message);
  
    const course = {
      id: courses.length + 1,
      name: req.body.name
    };
    courses.push(course);
    res.send(course);
  });
  
  app.put('/api/courses/:id', (req, res) => {
    const course = courses.find(c => c.id === parseInt(req.params.id));
    if (!course) return res.status(404).send('The course with the given ID was not found.');
  
    const { error } = validateCourse(req.body);
    if (error) return res.status(400).send(result.error.details[0].message);
  
    course.name = req.body.name;
    res.send(course);
  });
  
  app.delete('/api/courses/:id', (req, res) => {
    const course = courses.find(c => c.id === parseInt(req.params.id));
    if (!course) return res.status(404).send('The course with the given ID was not found.');
  
    const index = courses.indexOf(course);
    courses.splice(index, 1);
  
    res.send(course);
  });*/