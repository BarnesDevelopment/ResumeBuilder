docker-build:
	docker build -t sambobbarnes/resume-api .
	
docker-run:
	docker run -p 8080:80/tcp -p 10443:443/tcp sambobbarnes/resume-api
	
docker-build-and-run:
	docker build -t sambobbarnes/resume-api .
	docker run -p 8080:80/tcp -p 10443:443/tcp sambobbarnes/resume-api