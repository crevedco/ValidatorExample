using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using ValidatorExample.ValidationService;

namespace ValidatorExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new ServiceSoapClient();
            string[] phonesForValidate = { "+7 495 667 88 99", "+7 916 213 26 32" };

            for (int i = 0; i < phonesForValidate.Length; i++)
            {
                Console.WriteLine("Валидация одного номера:");
                Console.WriteLine(String.Format("Телефон для валидации: {0}", phonesForValidate[i]));

                var singleResult = client.Validate(string.Empty, phonesForValidate[i])[0];

                Console.WriteLine(String.Format("Код страны: {0}", singleResult.CountryCode));
                Console.WriteLine(String.Format("Телефонный код: {0}", singleResult.PhoneCode));
                Console.WriteLine(String.Format("Телефонный номер: {0}", singleResult.PhoneNum));
                Console.WriteLine(String.Format("Тип телефона: {0}", singleResult.PhoneType));
                Console.WriteLine(String.Format("Оператор связи: {0}", singleResult.Provider));

                Console.ReadLine();
            }

            Console.WriteLine("Пакетная валидация");

            Console.WriteLine("Формирование пакета записей для валидации");
            var batchList = new List<ValidationRequest>();
            for (int i = 0; i < phonesForValidate.Length; i++)
            {
                batchList.Add(new ValidationRequest { RecordId = Guid.NewGuid().ToString(), PhoneNum = phonesForValidate[i]});
            }

            Console.WriteLine("Передача задания на валидацию");
            var jobId = client.ValidateBatch(batchList.ToArray(), false, "Имярек");

            Console.WriteLine("Проверка состояния задания (выполнено или нет)");
            var status = client.GetJobStatus(jobId);
            while (status.Status != "DONE")
            {
                
                status = client.GetJobStatus(jobId);
                Console.WriteLine(String.Format("{0}: Статус задания '{1}'. Обработано телефонных номеров - {2}",
                                                    DateTime.Now, status.Status, status.Count));
                Thread.Sleep(TimeSpan.FromMinutes(1));
            }

            Console.WriteLine("Получение результатов валидации");
            var batchResult = client.GetValidationResult(jobId);

            for (int i = 0; i < batchResult.Length; i++)
            {
                Console.WriteLine();
                Console.WriteLine(String.Format("Идентификатор записи: {0}", batchResult[i].RecordId));
                Console.WriteLine(String.Format("Код страны: {0}", batchResult[i].CountryCode));
                Console.WriteLine(String.Format("Телефонный код: {0}", batchResult[i].PhoneCode));
                Console.WriteLine(String.Format("Телефонный номер: {0}", batchResult[i].PhoneNum));
                Console.WriteLine(String.Format("Тип телефона: {0}", batchResult[i].PhoneType));
                Console.WriteLine(String.Format("Оператор связи: {0}", batchResult[i].Provider));

                Console.ReadLine();
            }

        }
    }
}
