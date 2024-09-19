using System;
using System.Diagnostics.Metrics;
using System.Threading;

namespace ExemplosMultithreading
{
	class Program
	{
		// Propriedades compartilhadas para os exemplos		
		static Mutex mutex;
		static object lockObject;
		static Semaphore semaphore;
		static int contadorCompartilhado;
		static void Main(string[] args)
		{
			// Variável do Menu
			int opcao = -1;
			// Criando um menu de opções do programa
			while (opcao != 0)
			{
				Console.Clear();
				Console.WriteLine("==== Menu de Exemplos Multithreading ====");
				Console.WriteLine("1 - Exemplo de Threads simples");
				Console.WriteLine("2 - Exemplo com Mutex");
				Console.WriteLine("3 - Exemplo com Lock");
				Console.WriteLine("4 - Exemplo com Semáforo");
				Console.WriteLine("5 - Exemplo de Deadlock com tratamento de exceções");
				Console.WriteLine("0 - Sair");
				Console.Write("Escolha uma opção: ");

				if (!int.TryParse(Console.ReadLine(), out opcao))
				{
					Console.WriteLine("Opção inválida. Pressione qualquer tecla para continuar...");
					Console.ReadKey();
					continue;
				}

				// Lançando as classes especificas da opção de escolha
				switch (opcao)
				{
					case 1:
						ExemploThreadsSimples();
						break;
					case 2:
						ExemploComMutex();
						break;
					case 3:
						ExemploComLock();
						break;
					case 4:
						ExemploComSemaforo();
						break;
					case 5:
						ExemploDeadlock();
						break;
					case 0:
						Console.WriteLine("Encerrando o programa...");
						Environment.Exit(0);
						break;
					default:
						Console.WriteLine("Opção inválida. Pressione qualquer tecla para continuar...");
						Console.ReadKey();
						break;
				}
			}
		}

		// Exemplo 1: Threads simples
		static void ExemploThreadsSimples()
		{
			Console.Clear();
			Console.WriteLine("== Exemplo de Threads Simples ==");

			// Cria a primeira thread que executa o método ContarNumeros
			Thread thread1 = new Thread(ContarNumeros);
			thread1.Name = "Thread Numeros";
			thread1.Start();

			// Cria a segunda thread que executa o método MostrarAlfabeto
			Thread thread2 = new Thread(MostrarAlfabeto);
			thread2.Name = "Thread Alfabeto";
			thread2.Start();

			// Aguarda a conclusão das threads
			thread1.Join();
			thread2.Join();

			Console.WriteLine("As threads foram concluídas. Pressione qualquer tecla para voltar ao menu...");
			Console.ReadKey();
		}

		static void ContarNumeros()
		{
			// Cria um laço de contagem até 50
			for (int i = 1; i <= 50; i++)
			{
				//Informa o nome da thread e o número atual
				Console.WriteLine($"{Thread.CurrentThread.Name}: {i}");
				Thread.Sleep(100); // Pausa por 100 milissegundos
			}
		}

		static void MostrarAlfabeto()
		{
			// Cria um laço de contagem do alfabeto
			for (char c = 'A'; c <= 'Z'; c++)
			{
				//Informa o nome da thread e a letra atual
				Console.WriteLine($"{Thread.CurrentThread.Name}: {c}");
				Thread.Sleep(150); // Pausa por 150 milissegundos
			}
		}

		// Exemplo 2: Mutex
		static void ExemploComMutex()
		{
			Console.Clear();
			Console.WriteLine("== Exemplo com Mutex ==");

			// Cria um mutex para sincronização
			mutex = new Mutex();

			// Recurso compartilhado
			contadorCompartilhado = 0;

			// Cria a primeira thread que executa o método IncrementarContador
			Thread thread1 = new Thread(IncrementarComMutex);
			thread1.Name = "Thread 1";
			thread1.Start();

			// Cria a segunda thread que executa o método IncrementarContador
			Thread thread2 = new Thread(IncrementarComMutex);
			thread2.Name = "Thread 2";
			thread2.Start();

			// Aguarda a conclusão das threads
			thread1.Join();
			thread2.Join();

			Console.WriteLine("As threads foram concluídas.");
			Console.WriteLine($"Valor final do contador compartilhado: {contadorCompartilhado}");
			Console.WriteLine("Pressione qualquer tecla para voltar ao menu...");
			Console.ReadKey();
		}

		static void IncrementarComMutex()
		{
			for (int i = 0; i < 50; i++)
			{
				// Solicita a posse do mutex antes de acessar o recurso compartilhado
				mutex.WaitOne();

				// Seção crítica: acessa o recurso compartilhado
				contadorCompartilhado++;
				Console.WriteLine($"{Thread.CurrentThread.Name} incrementou o contador para {contadorCompartilhado}");

				// Libera o mutex após terminar a operação
				mutex.ReleaseMutex();

				// Pausa por 150 milissegundos
				Thread.Sleep(150);
			}
		}

		// Exemplo 3: Lock
		static void ExemploComLock()
		{
			Console.Clear();
			Console.WriteLine("== Exemplo com Lock ==");

			// Objeto para o lock
			lockObject = new object();

			// Recurso compartilhado
			contadorCompartilhado = 0;

			// Cria a primeira thread que executa o método
			Thread thread1 = new Thread(IncrementarComLock);
			thread1.Name = "Thread 1";
			thread1.Start();

			// Cria a segunda thread que executa o método
			Thread thread2 = new Thread(IncrementarComLock);
			thread2.Name = "Thread 2";
			thread2.Start();

			// Aguarda a conclusão das threads
			thread1.Join();
			thread2.Join();

			Console.WriteLine("As threads foram concluídas.");
			Console.WriteLine($"Valor final do contador compartilhado: {contadorCompartilhado}");
			Console.WriteLine("Pressione qualquer tecla para voltar ao menu...");
			Console.ReadKey();
		}

		static void IncrementarComLock()
		{
			for (int i = 0; i < 50; i++)
			{
				// Inicia a seção crítica usando lock
				lock (lockObject)
				{
					// Seção crítica: acessa o recurso compartilhado
					contadorCompartilhado++;
					Console.WriteLine($"{Thread.CurrentThread.Name} incrementou o contador para {contadorCompartilhado}");
				}

				Thread.Sleep(150); // Pausa por 150 milissegundos
			}
		}

		// Exemplo 4: Semáforo
		static void ExemploComSemaforo()
		{
			Console.Clear();
			Console.WriteLine("== Exemplo com Semáforo ==");

			// Cria um semáforo com contagem inicial de 1 e contagem máxima de 1 (funciona como um mutex)
			semaphore = new Semaphore(1, 1);

			// Recurso compartilhado
			contadorCompartilhado = 0;

			// Cria a primeira thread que executa o método
			Thread thread1 = new Thread(IncrementarComSemaforo);
			thread1.Name = "Thread 1";
			thread1.Start();

			// Cria a segunda thread que executa o método
			Thread thread2 = new Thread(IncrementarComSemaforo);
			thread2.Name = "Thread 2";
			thread2.Start();

			// Aguarda a conclusão das threads
			thread1.Join();
			thread2.Join();

			Console.WriteLine("As threads foram concluídas.");
			Console.WriteLine($"Valor final do contador compartilhado: {contadorCompartilhado}");
			Console.WriteLine("Pressione qualquer tecla para voltar ao menu...");
			Console.ReadKey();
		}

		static void IncrementarComSemaforo()
		{
			for (int i = 0; i < 50; i++)
			{
				// Executa com tratamento de excessões
				try
				{
					// Solicita acesso ao semáforo
					semaphore.WaitOne();

					// Seção crítica
					contadorCompartilhado++;
					Console.WriteLine($"{Thread.CurrentThread.Name} incrementou o contador para {contadorCompartilhado}");
				}
				catch (Exception ex) //Em caso de excessão
				{
					Console.WriteLine($"Ocorreu uma exceção na {Thread.CurrentThread.Name}: {ex.Message}");
				}
				finally
				{
					// Libera o semáforo
					semaphore.Release();
				}

				Thread.Sleep(150); // Pausa por 150 milissegundos
			}
		}

		// Exemplo 5: Deadlock com tratamento de exceções
		static void ExemploDeadlock()
		{
			Console.Clear();
			Console.WriteLine("== Exemplo de Deadlock com tratamento de exceções - Aguarde a tela atualizar ==");

			// Cria dois objetos que serão utilizados como recursos para sincronização
			object recursoA = new object();
			object recursoB = new object();

			// Cria a primeira thread que executa o método RecursoAB, passando os recursos A e B
			Thread thread1 = new Thread(() => RecursoAB(recursoA, recursoB));
			thread1.Name = "Thread 1";
			thread1.Start();

			// Cria a segunda thread que executa o método RecursoBA, passando os recursos A e B
			Thread thread2 = new Thread(() => RecursoBA(recursoA, recursoB));
			thread2.Name = "Thread 2";
			thread2.Start();

			// Aguarda a conclusão das threads com um timeout para evitar bloqueio indefinido
			if (!thread1.Join(10000)) // Aguarda até 10000 milissegundos (10 segundos) pela thread1
			{
				Console.WriteLine($"{thread1.Name} está demorando muito e pode estar em deadlock.");
			}
			if (!thread2.Join(5000)) // Aguarda até 10000 milissegundos (10 segundos) pela thread2
			{
				Console.WriteLine($"{thread2.Name} está demorando muito e pode estar em deadlock.");
			}

			//Pausa para simulur o tempo de deadlock
			Console.WriteLine("As Threads estão em deadlock.");
			Thread.Sleep(10000);

			// Informa ao usuário que o exemplo foi concluído
			Console.WriteLine("Fim do exemplo de deadlock. Pressione qualquer tecla para voltar ao menu...");
			Console.ReadKey();
		}

		static void RecursoAB(object recursoA, object recursoB)
		{
			try
			{
				// Tenta adquirir o bloqueio no recursoA

				lock (recursoA)
				{
					Console.WriteLine($"{Thread.CurrentThread.Name} bloqueou recurso A");
					// Pausa para simular processamento e permitir que a outra thread bloqueie recursoB
					Thread.Sleep(150);

					// Dentro do bloqueio de recursoA, tenta adquirir o bloqueio em recursoB
					Console.WriteLine($"{Thread.CurrentThread.Name} tentando bloquer recursos B");
					lock (recursoB)
					{
						Console.WriteLine($"{Thread.CurrentThread.Name} bloqueou recursos A e B");
						// Aqui teria o processamento que necessita dos dois recursos
					}
				}
			}
			catch (Exception ex)
			{
				// Caso ocorra alguma exceção, ela é capturada e exibida no console
				Console.WriteLine($"Exceção na {Thread.CurrentThread.Name}: {ex.Message}");
			}
		}

		// Método executado pela Thread 2, tentando bloquear recursos na ordem B -> A
		static void RecursoBA(object recursoA, object recursoB)
		{
			try
			{
				// Tenta adquirir o bloqueio no recursoB				
				lock (recursoB)
				{
					Console.WriteLine($"{Thread.CurrentThread.Name} bloqueou recurso B");
					// Pausa para simular processamento e permitir que a outra thread bloqueie recursoA
					Thread.Sleep(150);
					// Dentro do bloqueio de recursoB, tenta adquirir o bloqueio em recursoA
					Console.WriteLine($"{Thread.CurrentThread.Name} tentando bloquer recursos A");
					lock (recursoA)
					{
						Console.WriteLine($"{Thread.CurrentThread.Name} bloqueou recursos B e A");
						// Aqui teria o processamento que necessita dos dois recursos
					}
				}
			}
			catch (Exception ex)
			{
				// Captura e exibe qualquer exceção que ocorra
				Console.WriteLine($"Exceção na {Thread.CurrentThread.Name}: {ex.Message}");
			}
		}
	}
}
