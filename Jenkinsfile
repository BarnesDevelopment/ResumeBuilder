pipeline {
  agent {label 'web-host'}
  stages {
    stage('Checkout') {
      steps {
        checkout scm
      }
    }
    
    stage('Build') {
      steps {
        script {
          def app = docker.build("app:${BUILD_VERSION}")
        }
      }
    }
    
    stage('Upload') {
      steps {
        script {
          def build_version = input (id: 'Build_version', message: 'Build version:', parameters: [string(description: 'Build Version Tag', name: 'BUILD_VERSION', trim: true)])
          docker.withRegistry('https://registry.hub.docker.com', 'docker_hub') {
            app.push(build_version)
            app.push("latest")
          }
        }
      }
    }
  }
}

