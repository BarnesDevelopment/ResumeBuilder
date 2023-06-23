const Post = require('../models/Post');

exports.getAllPosts = async (req, res, next) => {
  try {
    const [posts, _] = await Post.findAll();
    res.status(200).json({count: posts.length, posts});
  } catch (error) {
    console.log(error);
    next(error);
  }
}

exports.createNewPost = async (req, res, next) => {
  try {
    let { title, body } = req.body;
    let post = new Post(title, body);
    post = await post.save();
    res.status(201).json({message: "Post created"});
  } catch(error) {
    console.log(error);
    next(error);
  }

}

exports.getPostById = async (req, res, next) => {
  try {
    let [post, _] = await Post.findById(req.params.id);
    res.status(200).json({post: post[0]});
  } catch(error) {
    console.log(error);
    next(error);
  }
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