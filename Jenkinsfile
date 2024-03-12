pipeline {
  agent none
  stages {
    stage('Checkout') {
      agent { label 'docker' }
      steps {
        checkout scm
      }
    }
  
    stage('Build and upload') {
      agent { label 'docker' }
      input {
          message "Build version:"
          parameters {
              string(name: 'BUILD_VERSION', defaultValue: '', description: 'Build Version Tag', trim: true)
          }
      }
      
      steps {
        app = docker.build("sambobbarnes/resume-api")
        docker.withRegistry('https://registry.hub.docker.com', 'docker-hub') {
          app.push("${BUILD_VERSION}")
          app.push("latest")
        }
      
//         sh 'echo ${BUILD_VERSION}'
//         sh 'docker build -t sambobbarnes/resume-api:${BUILD_VERSION} .'
//         sh 'docker build -t sambobbarnes/resume-api:latest .'
//         
//         sh 'docker push sambobbarnes/resume-api:${BUILD_VERSION}'
//         sh 'docker push sambobbarnes/resume-api:latest'
      }
    }
    
    stage('Deploy') {
      agent { label 'web-host' }
      steps {
        sh 'docker-compose up -d --force-recreate'
      }
    }
  }
}