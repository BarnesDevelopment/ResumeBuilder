node {
  def app
  input {
      message "Build version:"
      parameters {
          string(name: 'BUILD_VERSION', defaultValue: '', description: 'Build Version Tag', trim: true)
      }
    }
  
  stage('Checkout') {
    checkout scm
  }
  
  stage('Build image') {
    app = docker.build("sambobbarnes/resume-api")
  }
  
  stage('Upload image') {
    docker.withRegistry('https://registry.hub.docker.com', 'docker-hub') {
      app.push("${BUILD_VERSION}")
      app.push("latest")
    }
  }
}