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
      steps {
        script {
            def BUILD_VERSION
            def userInput = input (id: 'buildVersion', message: 'Build version:', parameters: [string(description: 'Build Version Tag', name: 'BUILD_VERSION', trim: true)])
            export BUILD_VERSION=userInput['BUILD_VERSION']
        }
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