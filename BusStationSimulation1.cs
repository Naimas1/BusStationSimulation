namespace BusStationSimulation
{
    // Класс для представления автобуса
    class Bus
    {
        public int Number { get; } // Номер автобуса
        public int Capacity { get; } // Вместимость автобуса
        public int Passengers { get; private set; } // Текущее количество пассажиров в автобусе
        public event EventHandler<int> BusArrived; // Событие прибытия автобуса

        public Bus(int number, int capacity)
        {
            Number = number;
            Capacity = capacity;
        }

        // Метод для обработки прибытия автобуса на остановку
        public void Arrive(int passengersOnBoard)
        {
            // Если автобус прибыл с пассажирами на борту, добавляем их к текущему количеству пассажиров
            Passengers = passengersOnBoard;

            // Генерируем случайное количество пассажиров, которые ждут этот автобус
            Random rand = new Random();
            int newPassengers = rand.Next(1, Capacity - Passengers + 1);

            // Вызываем событие, передавая количество прибывших пассажиров
            BusArrived?.Invoke(this, newPassengers);

            // Обновляем количество пассажиров в автобусе
            Passengers += newPassengers;
        }
    }

    class Programs
    {
        static int totalPassengers = 0; // Общее количество пассажиров на остановке
        static Dictionary<int, int> passengersToBus = new Dictionary<int, int>(); // Количество пассажиров, желающих сесть в каждый автобус
        static object lockObj = new object(); // Объект для синхронизации доступа к общему ресурсу

        // Метод для обработки прибытия автобуса
        static void BusArrivedHandler(object sender, int newPassengers)
        {
            // Получаем текущий автобус
            Bus bus = (Bus)sender;
            int busNumber = bus.Number;

            // Синхронизируем доступ к общему ресурсу
            lock (lockObj)
            {
                // Если есть пассажиры, которые ждут этот автобус, загружаем их в автобус
                if (passengersToBus.ContainsKey(busNumber))
                {
                    int passengersToBoard = Math.Min(passengersToBus[busNumber], bus.Capacity - bus.Passengers);
                    bus.Passengers += passengersToBoard;
                    totalPassengers -= passengersToBoard;
                    passengersToBus.Remove(busNumber);
                }
            }

            Console.WriteLine($"Bus {bus.Number} arrived with {newPassengers} new passengers. Passengers on stop: {totalPassengers}");
        }

        static void Main(string[] args)
        {
            // Создаем автобусы с вместимостью 50 пассажиров
            Bus bus1 = new Bus(175, 50);
            Bus bus2 = new Bus(176, 50);

            // Подписываемся на события прибытия автобусов
            bus1.BusArrived += BusArrivedHandler;
            bus2.BusArrived += BusArrivedHandler;

            // Запускаем потоки для работы автобусов
            Thread busThread1 = new Thread(() =>
            {
                while (true)
                {
                    bus1.Arrive(10); // Указываем количество пассажиров, приехавших на остановку вместе с автобусом
                    Thread.Sleep(2000); // Задержка между прибытиями автобусов
                }
            });
            busThread1.Start();

            Thread busThread2 = new Thread(() =>
            {
            while (true)
            {
                bus2.Arrive(5); // Указываем количество пассажиров, приехавших на остановку вместе с автобусом
                Thread.Sleep(2000); // Задержка между прибытиями авт
            };
            
            busThread2.Start();

            // Пользовательский интерфейс для добавления пассажиров на остановку
            Thread userInputThread = new Thread(() =>
            {
                Random rand = new Random();
                while (true)
                {
                    // Генерация случайного числа пассажиров
                    int newPassengers = rand.Next(1, 21);
                    Console.WriteLine($"New passengers arrived at the bus stop: {newPassengers}");
                    totalPassengers += newPassengers;

                    // Синхронизируем доступ к общему ресурсу
                    lock (lockObj)
                    {
                        // Распределение пассажиров между автобусами
                        if (totalPassengers > 0)
                        {
                            passengersToBus[175] = rand.Next(1, totalPassengers + 1);
                            passengersToBus[176] = totalPassengers - passengersToBus[175];
                        }
                    }

                    Thread.Sleep(5000); // Задержка между прибытием пассажиров
                }
            });
            userInputThread.Start();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            }
    }
}
