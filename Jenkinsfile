node {
  def app
  def build_version
  
  stage('Checkout') {
    checkout scm
  }
  
  stage('Build image') {
    app = docker.build("sambobbarnes/resume-api")
  }
  
  stage('Upload image') {
    build_version = input (id: 'Build_version', message: 'Build version:', parameters: [string(description: 'Build Version Tag', name: 'BUILD_VERSION', trim: true)])
    docker.withRegistry('https://registry.hub.docker.com', 'docker_hub') {
      app.push(build_version)
      app.push("latest")
    }
  }
}