const express = require('express');
const exampleControllers = require('../controllers/exampleControllers');
const router = express.Router();

// @route GET && POST - /api/ex/
router
    .route('/')
    .get(exampleControllers.getAllPosts)
    .post(exampleControllers.createNewPost);

router.route("/:id").get(exampleControllers.getPostById);

module.exports = router;