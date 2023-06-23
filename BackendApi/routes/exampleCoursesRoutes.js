const express = require('express');
const controllers = require('../controllers/exampleCoursesControllers');
const router = express.Router();

// @route GET && POST - /api/ex/
router
    .route('/')
    .get(controllers.getAllCourses)
    .post(controllers.addNewCourse);

router.route("/:id").get(controllers.getCourseByID).put(controllers.modifyExistingCourse).delete(controllers.removeCourse);

router.route("/:year/:month").get(controllers.getCoursesByDate);

module.exports = router;