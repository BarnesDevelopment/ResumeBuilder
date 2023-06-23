FROM node:alpine
LABEL maintainer="dev@barnes-development.com"
WORKDIR /home/app
COPY ./ /home/app
RUN npm install
EXPOSE 3000
CMD ["npm", "start"]