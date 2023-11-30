# MensageriaCSVRabbitMQ

Solução de exemplo de Mensageria para exportação de CSV usando o RabbitMQ.

Para rodar um servidor para troca de mensagens:

docker run -p 15672:15672 -p 5672:5672 masstransit/rabbitmq
