pipeline {
  agent none
  parameters {
    string(name: 'BUILD_VERSION', defaultValue: '', description: 'Build Version Tag')
  }
  environment {
    def BUILD_VERSION = "${params.BUILD_VERSION}"
  }
  stages {
    stage('Checkout') {
      agent { label 'docker' }
      steps {
        checkout scm
      }
    }
  
    stage('Build and upload') {
      agent { label 'docker' }
      
      steps {
        sh 'echo ${BUILD_VERSION}'
        sh 'docker build -t sambobbarnes/resume-api:${BUILD_VERSION} .'
        sh 'docker build -t sambobbarnes/resume-api:latest .'
        
        sh 'docker push -a sambobbarnes/resume-api:${BUILD_VERSION}'
        sh 'docker push -a sambobbarnes/resume-api:latest'
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